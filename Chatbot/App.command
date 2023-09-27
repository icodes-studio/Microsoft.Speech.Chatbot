<?xml version="1.0" encoding="UTF-8" ?>
<grammar version="1.0" xml:lang="ko-KR" xmlns="http://www.w3.org/2001/06/grammar" tag-format="semantics-ms/1.0" root="request">

    <rule id="request" scope="public">
        <one-of>
            <item><tag>1000</tag><ruleref uri="#you"/><ruleref uri="#hi"/></item>
            <item><tag>1001</tag><ruleref uri="#you"/>결혼했어?</item>
            <item><tag>1002</tag><ruleref uri="#you"/>몇 살이야?</item>
            <item><tag>1003</tag><ruleref uri="#thanks"/></item>
            <item><tag>1004</tag><ruleref uri="#you"/>사랑해.</item>
            <item><tag>1004</tag><ruleref uri="#you"/>사랑해요.</item>
            <item><tag>1004</tag><ruleref uri="#you"/>사랑합니다.</item>
            <item><tag>1005</tag><ruleref uri="#insult"/></item>
            <item><tag>1006</tag><ruleref uri="#you"/>여친 있어?</item>
            <item><tag>1006</tag><ruleref uri="#you"/>여자친구 있어?</item>
            <item><tag>1006</tag><ruleref uri="#you"/>남친 있어?</item>
            <item><tag>1006</tag><ruleref uri="#you"/>남자친구 있어?</item>
            <item><tag>1007</tag><ruleref uri="#you"/>여기서 뭐해?</item>
            <item><tag>1008</tag><ruleref uri="#you"/>이름이 뭐니?</item>
            <item><tag>1009</tag><ruleref uri="#you"/>남자야 여자야?</item>

            <item><tag>power</tag><ruleref uri="#power"/></item>
            <item><tag>notepad</tag><ruleref uri="#notepad"/><ruleref uri="#open"/></item>
            <item><tag>explorer</tag><ruleref uri="#explorer"/><ruleref uri="#open"/></item>

            <item><tag>yes</tag>응</item>
            <item><tag>yes</tag>어</item>
            <item><tag>yes</tag>엉</item>
            <item><tag>yes</tag>네</item>
            <item><tag>yes</tag>네 네</item>
            <item><tag>yes</tag>내</item>
            <item><tag>yes</tag>그래</item>
            <item><tag>yes</tag>그래요</item>
            <item><tag>yes</tag>좋아</item>
            <item><tag>yes</tag>좋아요</item>
            <item><tag>yes</tag>어 그래</item>
            <item><tag>no</tag>아니</item>
            <item><tag>no</tag>아니요</item>
            <item><tag>no</tag>안돼</item>
            <item><tag>no</tag>됐어</item>
            <item><tag>no</tag>대써</item>
        </one-of>
    </rule>

    <rule id="answer" scope="private">
        <one-of>
            <item><tag>1000</tag>안녕하세요. 챗봇입니다.</item>
            <item><tag>1001</tag>아니오, 좋은 사람. 아니 봇 있으면 소개해 주세요.</item>
            <item><tag>1002</tag>먹을 만큼 먹었어요.</item>
            <item><tag>1003</tag>별 말씀을요. 좋은 하루 보내세요.</item>
            <item><tag>1004</tag>저도 사랑합니다.</item>
            <item><tag>1005</tag>저 상처받았어요.</item>
            <item><tag>1006</tag>모태쏠로랍니다. 소개 좀 시켜 주세요.</item>
            <item><tag>1007</tag>마이크로소프트의 스피취 에스디케이를 이용한 챗봇 데모를 보여드리고 있어요.</item>
            <item><tag>1008</tag>이름 없어요. 그냥 흔한 챗봇입니다.</item>
            <item><tag>1009</tag>저도 궁금하네요.</item>
            <item><tag>unknown</tag>잘 이해하지 못했어요.</item>
        </one-of>
    </rule>

    <rule id="you" scope="private">
        <one-of>
            <item>챗봇</item>
            <item>로봇</item>
            <item>너</item>
            <item> </item>
        </one-of>
    </rule>

    <rule id="hi" scope="private">
        <one-of>
            <item>하이</item>
            <item>안녕</item>
            <item>안녕하세요</item>
            <item>반가워</item>
        </one-of>
    </rule>

    <rule id="thanks" scope="private">
        <one-of>
            <item>땡큐</item>
            <item>감사</item>
            <item>감사해요</item>
            <item>감사합니다</item>
            <item>고마워</item>
            <item>고마워요</item>
            <item>고맙습니다</item>
        </one-of>
    </rule>

    <rule id="insult" scope="private">
        <one-of>
            <item>킹받네</item>
            <item>개짜증나</item>
            <item>멍청이야</item>
            <item>멍청이</item>
            <item>멍충이</item>
            <item>바보야</item>
            <item>바보</item>
            <item>똥개</item>
            <item>똥개새끼</item>
            <item>똥개새끼야</item>
            <item>또라이</item>
            <item>또라이야</item>
            <item>또라이새끼</item>
            <item>또라이새끼야</item>
            <item>메롱</item>
            <item>븅신</item>
            <item>병신</item>
            <item>병신새끼</item>
            <item>등신</item>
            <item>등신새끼</item>
            <item>씨발</item>
            <item>씨부랄</item>
            <item>씨발년아</item>
            <item>씨발놈아</item>
            <item>개새끼</item>
            <item>개새끼야</item>
            <item>씨발새끼</item>
            <item>좆같은새끼</item>
            <item>지랄하네</item>
            <item>좆까</item>
        </one-of>
    </rule>

    <rule id="power" scope="private">
        <one-of>
            <item>절전모드</item>
            <item>절전모드로 들어가</item>
            <item>꺼져</item>
            <item>파워오프</item>
            <item>컴퓨터 꺼</item>
        </one-of>
    </rule>

    <rule id="notepad" scope="private">
        <one-of>
            <item>메모장</item>
            <item>노트패드</item>
        </one-of>
    </rule>

    <rule id="explorer" scope="private">
        <one-of>
            <item>탐색기</item>
            <item>파일 탐색기</item>
            <item>폴더</item>
            <item>익스플로러</item>
        </one-of>
    </rule>
  
    <rule id="open" scope="private">
        <one-of>
            <item>띄워줘</item>
            <item>열어줘</item>
            <item>실행 시켜줘</item>
            <item>띄워</item>
            <item>열어</item>
            <item>실행</item>
            <item>실행 시켜</item>
            <item> </item>
        </one-of>
    </rule>
  
</grammar>